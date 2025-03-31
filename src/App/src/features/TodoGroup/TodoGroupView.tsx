import { useParams } from "react-router-dom";
import { useTodoGroupStore } from "./todoGroupStore";
import { Descriptions } from "antd";
import { useEffect } from "react";
import { TodoGroupService } from "./todoGroupService";

export const TodoGroupView = () => {
  const { currentTodoGroup, setTodoGroupCurrent } = useTodoGroupStore();
  const { id } = useParams();
  useEffect(() => {
    if (id) {
      TodoGroupService.fetchTodoGroupById(id).then((res) =>
        setTodoGroupCurrent(res.data, null)
      );
    }
  }, [id, setTodoGroupCurrent]);

  return (
    <>
      {currentTodoGroup && (
        <Descriptions column={1} bordered>
          <Descriptions.Item label="Name">
            {currentTodoGroup.title}
          </Descriptions.Item>
          <Descriptions.Item label="Colour">
            <div
              style={{
                display: "inline-block",
                width: 24,
                height: 24,
                borderRadius: "50%",
                backgroundColor: currentTodoGroup.colour,
                border: "1px solid #ccc",
              }}
            />
          </Descriptions.Item>
        </Descriptions>
      )}
    </>
  );
};
