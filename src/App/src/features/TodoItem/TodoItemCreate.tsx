/* eslint-disable @typescript-eslint/no-explicit-any */
import React from "react";
import { Modal, Form, Input, notification, Select } from "antd";
import { TodoGroupService } from "./todoItemService";
import { useTodoGroupStore } from "./todoItemStore";

interface TodoGroupModalProps {
  visible: boolean;
  onClose: () => void;
}

const TodoGroupCreate: React.FC<TodoGroupModalProps> = ({
  visible,
  onClose,
}) => {
  const [form] = Form.useForm();
  const { fetchTodoGroups } = useTodoGroupStore();

  const handleSaveTodoGroup = () => {
    form.validateFields().then(async (values) => {
      try {
        await TodoGroupService.createTodoGroup(values);
        notification.success({ message: "Operation successful!" });
        onClose();
        form.resetFields();
        await fetchTodoGroups();
      } catch (error: any) {
        console.error("Error creating todo group:", error);
        notification.error({ message: "Failed to create todo group!" });
      }
    });
  };

  return (
    <Modal
      title={"Add New Todo List"}
      open={visible}
      onCancel={onClose}
      onOk={handleSaveTodoGroup}
    >
      <Form form={form} layout="vertical">
        <Form.Item
          label="Todo List Title"
          name="title"
          rules={[
            { required: true, message: "Please enter the todo list title!" },
          ]}
        >
          <Input placeholder="Enter todo list title" />
        </Form.Item>
        <Form.Item
          label="Select Colour"
          name="colour"
          rules={[{ required: true, message: "Please select a colour!" }]}
        >
          <Select optionLabelProp="label">
            {TodoGroupService.getColours().map((colour) => (
              <Select.Option
                key={colour.value}
                value={colour.value}
                label={
                  <span style={{ display: "flex", alignItems: "center" }}>
                    <span
                      style={{
                        display: "inline-block",
                        width: 20,
                        height: 20,
                        backgroundColor: colour.value,
                        marginRight: 10,
                        borderRadius: "50%",
                      }}
                    />
                    {colour.label}
                  </span>
                }
              >
                <span style={{ display: "flex", alignItems: "center" }}>
                  <span
                    style={{
                      display: "inline-block",
                      width: 20,
                      height: 20,
                      backgroundColor: colour.value,
                      marginRight: 10,
                      borderRadius: "50%",
                    }}
                  />
                  {colour.label}
                </span>
              </Select.Option>
            ))}
          </Select>
        </Form.Item>
      </Form>
    </Modal>
  );
};

export default TodoGroupCreate;
