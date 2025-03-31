import { useNavigate, useParams } from "react-router-dom";
import { useTodoGroupStore } from "./todoGroupStore";
import { Card, Descriptions, Button } from "antd";
import { useEffect } from "react";
import { TodoGroupService } from "./todoGroupService";

export const TodoGroupView = () => {
  const { currentTodoGroup, setTodoGroupCurrent } = useTodoGroupStore();
  const navigate = useNavigate();
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
        <Card
          title="Todo List Details"
          extra={
            <Button type="default" onClick={() => navigate("/todo-list")}>
              Back
            </Button>
          }
        >
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
        </Card>
      )}
    </>
  );
};
